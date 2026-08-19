import ClassItem from "../components/ClassItem.jsx";
import LoadingCircle from "../components/LoadingCircle.jsx";

function MainPage({data, afApiSignupForClass, afApiSignoutFromClass, fUpdateData, showOnlySignedUp}) {

    const items = showOnlySignedUp && data ?
        data.filter(item => item.youSignedUp) :
        data;

    return (
        <div className={"w-100 p-2 bg-dark"}>
            {
                items &&
                (
                    items.map((item, index) => (
                        <ClassItem key={"ci-"+index} data={item}
                               afApiSignUp={afApiSignupForClass}
                               afApiSignOut={afApiSignoutFromClass}
                               fUpdateData={fUpdateData}
                               index={index}/>
                    ))
                )
            }
            {
                !items &&
                <LoadingCircle className={"mt-5"}/>
            }
        </div>
    )
}
export default MainPage;