import axios from "axios";

const ApiInfo = {
    adress: "http://localhost",
    port: "2137",
    version: "v1",
    getUrl: () => ApiInfo.adress + ":" + ApiInfo.port + "/api/" + ApiInfo.version,
}

const Api = axios.create({
    baseURL: ApiInfo.getUrl(),
});

Api.interceptors.request.use(config => {
    const token = localStorage.getItem("jwt");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

Api.interceptors.response.use(config => config, async (error) => {
    const originalReq = error.config;
    if(error.response?.status === 401 &&
        !originalReq.url.includes("/auth/refresh") &&
        !originalReq._retry)
    {
        originalReq._retry = true;
        const refreshToken = localStorage.getItem("refresh");
        if(!refreshToken)
        {
            return Promise.reject(error);
        }

        try
        {
            const {data} = await Api.post("/auth/refresh", {RefreshToken: refreshToken});

            localStorage.setItem("jwt", data.jwtToken);
            localStorage.setItem("refresh", data.refreshToken);

            originalReq.headers.Authorization = `Bearer ${data.jwtToken}`;
            return Api(originalReq);
        }
        catch (refreshError)
        {
            localStorage.clear();
            window.location.href = "/";
            return Promise.reject(refreshError);
        }
    }

    return Promise.reject(error);
});

export {Api, ApiInfo};