import {Spinner} from "react-bootstrap";

function LoadingCircle()
{
    return (
        <div className={"w-100 h-100 d-flex justify-content-center align-items-center"}>
            <Spinner variant={"primary"} style={{scale: 10}}/>
        </div>
    )
}
export default LoadingCircle;