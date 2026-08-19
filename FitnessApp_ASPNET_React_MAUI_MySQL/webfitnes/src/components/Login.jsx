import {Alert, Button, FloatingLabel, FormControl} from "react-bootstrap";
import { useState } from "react";
import validator from "validator/es/index.js";
import ReqRegister from "../apiTemplates/ReqRegister.js";
import ReqLogin from "../apiTemplates/ReqLogin.js";

function Login(props) {

    const {afApiLogin, afApiRegister} = props;
    const [isSigningUp, setIsSigningUp] = useState(false);

    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [repeatPassword, setRepeatPassword] = useState("");
    const [username, setUsername] = useState("");
    const [alertMessage, setAlertMessage] = useState({error: true, message: null});
    const [logging, setLogging] = useState(false);

    function registerOrLoginStr(invert)
    {
        return !isSigningUp ^ invert ? "Zaloguj się" : "Zarejestruj się";
    }
    async function Submit(evt)
    {
        evt.preventDefault();
        if(password !== repeatPassword)
        {
            setAlertMessage({message: "Hasła muszą być takie same!", error: true});
            return;
        }
        setLogging(true);
        if(isSigningUp)
        {
            const res = await afApiRegister(new ReqRegister(login, password, username));
            setLogging(false);
            if(res.error)
            {
                setAlertMessage({message: res.error, error: true});
                return;
            }
            setAlertMessage({message: "Zarejestrowano pomyślnie!", error: false});
        }
        else
        {
            const res = await afApiLogin(new ReqLogin(login, password));
            setLogging(false);
            if(res.error)
            {
                setAlertMessage({message: res.error, error: true});
                return;
            }
            localStorage.setItem("jwt", res.jwtToken);
            localStorage.setItem("refresh", res.refreshToken);
            setAlertMessage({message: null, error: true});
        }
    }

    return (
        <div className={"rounded-4 w-75 bg-dark-subtle " +
            "border-3 border-light border text-center container " +
            "p-3"}>
            <h1 className={"h1 mb-3 text-dark font-monospace"}>{registerOrLoginStr(false)}</h1>

            <FloatingLabel label={"Email"} className={"my-2"}>
                <FormControl type={"email"} placeholder={"Email"} isInvalid={!validator.isEmail(login)}
                value={login} onChange={(e) => setLogin(e.target.value.toLowerCase())}
                required/>
                <FormControl.Feedback type={"invalid"}>Podany email jest niepoprawny</FormControl.Feedback>
            </FloatingLabel>

            <FloatingLabel label={"Hasło"} className={"my-2"}>
                <FormControl type={"password"} placeholder={"Hasło"}
                value={password} onChange={(e) => setPassword(e.target.value)}
                isInvalid={isSigningUp && password !== repeatPassword}
                required/>
            </FloatingLabel>

            {isSigningUp && <FloatingLabel label={"Powtórz hasło"} className={"my-2"}>
                <FormControl type={"password"} placeholder={"Powtórz hasło"}
                value={repeatPassword} onChange={(e) => setRepeatPassword(e.target.value)}
                isInvalid={isSigningUp && password !== repeatPassword}/>
                <FormControl.Feedback type={"invalid"}>Hasła muszą być takie same</FormControl.Feedback>
            </FloatingLabel>}

            {isSigningUp && <FloatingLabel label={"Nazwa użytkownika"} className={"my-2"}>
                <FormControl type={"text"} placeholder={"Nazwa użytkownika"}
                value={username} onChange={(e) => setUsername(e.target.value)}
                isInvalid={isSigningUp && (username.length < 3 || username.length > 60)}/>
                <FormControl.Feedback type={"invalid"}>Nazwa użytkownika musi mieścić się w przedziale (3-60)</FormControl.Feedback>
            </FloatingLabel>}

            <Button type={"submit"} className={"w-100 my-2"}
                    onClick={Submit} disabled={logging}>{registerOrLoginStr(false)}</Button>
            <p className={"m-0 p-0"}>
                {
                    isSigningUp ?
                    "Masz już konto?" :
                    "Nie masz jeszcze konta?"
                }
                &nbsp;
                <a onClick={() => setIsSigningUp((prevState) => !prevState)}
                    className={"text-primary"} style={{cursor: "pointer"}}>
                    {registerOrLoginStr(true)}
                </a>
            </p>
            {
                alertMessage.message && (
                    <Alert variant={alertMessage.error ? "danger" : "success"} className={"mt-2"}>
                        {alertMessage.message}
                    </Alert>
                )
            }
        </div>
    );
}

export default Login;