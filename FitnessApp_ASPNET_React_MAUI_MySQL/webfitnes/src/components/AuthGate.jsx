import {useEffect, useState} from "react";
import {Api} from "../Api.js";
import Login from "./Login.jsx";
import LoadingCircle from "./LoadingCircle.jsx";


function AuthGate({children, afApiLogin, afApiRegister, authorized, fSetAuthorized}) {
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchAuthorize = async () => {
            const refreshToken = localStorage.getItem('refresh');
            if(!refreshToken)
            {
                localStorage.clear();
                fSetAuthorized(false);
                setLoading(false);
                return;
            }

            const request = {
                RefreshToken: refreshToken,
            }
            try {
                const response = await Api.post("/auth/refresh", request);

                if (!response?.data?.jwtToken || !response?.data?.refreshToken) {
                    throw new Error("Invalid refresh response");
                }

                localStorage.setItem("jwt", response.data.jwtToken);
                localStorage.setItem("refresh", response.data.refreshToken);
                fSetAuthorized(true);
            } catch (err) {
                console.error("Refresh failed:", err);
                localStorage.clear();
                fSetAuthorized(false);
            } finally {
                setLoading(false);
            }
        }
        fetchAuthorize();
    }, []);

    if (loading) return <LoadingCircle/>;
    if(!authorized) return (
        <div className="w-100 h-100 d-flex align-items-center justify-content-center">
            <Login afApiLogin={afApiLogin} afApiRegister={afApiRegister}/>
        </div>
    );

    return children;
}

export default AuthGate;