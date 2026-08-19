import 'bootstrap/dist/css/bootstrap.css';
import Login from "./components/Login.jsx";
import {Api} from "./Api";
import AuthGate from "./components/AuthGate.jsx";
import LoadingCircle from "./components/LoadingCircle.jsx";
import {Button} from "react-bootstrap";
import {useEffect, useState} from "react";
import MainPage from "./pages/MainPage.jsx";

const PagesEnum = Object.freeze({
    MAIN: "MainPage",
    MY_CLASSES: "MyClassesPage",
});

function App() {
    const [authorized, setAuthorized] = useState(false);
    const [allClasses, setAllClasses] = useState(null);
    const [selectedPage, setSelectedPage] = useState(PagesEnum.MAIN);
    const [refresherCounter, setRefresherCounter] = useState(-2147483648);

    async function ApiLogout() {
        try {
            await Api.post("/auth/logout");
            localStorage.clear();
            setAuthorized(false);
            setAllClasses(null);
            setSelectedPage(PagesEnum.MAIN);
            return true;
        } catch (er) {
            return {error: er.message};
        }
    }

    async function ApiRegister(data) {
        try {
            const res = await Api.post("/auth/register", data);
            return res.data;
        } catch (er) {
            if (er.response.status === 400)
                return {error: "Wprowadzony email jest nie poprawny"};
            if (er.response.status === 409)
                return {error: "Istnieje już konto o tym adresie Email"};
            return {error: er.message};
        }
    }

    async function ApiLogin(data) {
        try {
            const res = await Api.post("/auth/login", data);
            setAuthorized(true);
            return res.data;
        } catch (er) {
            setAuthorized(false);
            if (er.response.status === 400)
                return {error: "Nieprawidłowe email lub hasło."}
            return {error: er.message};
        }
    }

    async function ApiGetAllClasses() {
        try {
            const res = await Api.get("/classes");
            return res.data;
        } catch (er) {
            return {error: er.message};
        }
    }

    async function ApiSignupForClass(classId) {
        try {
            await Api.post(`/classes/${classId}/signup`, {});
            return true;
        } catch (er) {
            if(er.response.status === 406)
                return {error: "Brak wolnych miejsc."};
            return {error: er.message};
        }
    }

    async function ApiSignoutFromClass(classId) {
        try {
            await Api.post(`/classes/${classId}/leave`, {});
            return true;
        } catch (er) {
            return {error: er.message};
        }
    }

    const incrementRefresh = (evt) => {
        evt.preventDefault();
        setRefresherCounter(prevState => prevState + 1);
    }


    useEffect(() => {
        if(!authorized)
        { return; }

        async function getAllClasses() {
            const res = await ApiGetAllClasses();
            setAllClasses(res);
        }
        getAllClasses();
    }, [refresherCounter, authorized])

    return (<div className={"bg-dark wh-100 vh-100"}>
        <AuthGate afApiLogin={ApiLogin} afApiRegister={ApiRegister} authorized={authorized}
                  fSetAuthorized={setAuthorized}>
            <div className="bg-black d-flex align-items-center justify-content-between mb-3 p-2 rounded-bottom-4">

                <span>
                    <Button className={"mx-2"} variant={selectedPage === PagesEnum.MAIN ? "info" : "outline-info"}
                            onClick={event => {
                                event.preventDefault();
                                setSelectedPage(PagesEnum.MAIN);
                            }}>Strona główna</Button>
                    <Button className={"mx-2"} variant={selectedPage === PagesEnum.MY_CLASSES ? "info" : "outline-info"}
                            onClick={event => {
                                event.preventDefault();
                                setSelectedPage(PagesEnum.MY_CLASSES);
                            }}>Moje zajęcia</Button>
                </span>
                <Button className={"mx-2"} variant={"outline-success"}
                        onClick={incrementRefresh}>
                    Odświerz
                </Button>
                <Button className={"mx-2"} variant={"outline-danger"}
                        onClick={event => {
                            event.preventDefault();
                            ApiLogout();
                        }}>Wyloguj się</Button>
            </div>
            <MainPage data={allClasses} showOnlySignedUp={selectedPage === PagesEnum.MY_CLASSES}
                      fUpdateData={setAllClasses} afApiSignupForClass={ApiSignupForClass}
                      afApiSignoutFromClass={ApiSignoutFromClass}/>
        </AuthGate>
    </div>);
}

export default App;
