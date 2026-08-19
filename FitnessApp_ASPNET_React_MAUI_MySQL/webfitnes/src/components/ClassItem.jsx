import { Card, Button, Row, Col } from "react-bootstrap";
import { useState } from "react";

function ClassItem({ data, fUpdateData, afApiSignOut, afApiSignUp, index }) {
    const [isLoading, setIsLoading] = useState(false);
    const signedUp = data.youSignedUp;

    const getNextDate = (start, repetition) => {
        let next = new Date(start);
        const now = new Date();

        if (next > now || !repetition || repetition === "None") return next;

        while (next < now) {
            switch (repetition) {
                case "Daily": next.setDate(next.getDate() + 1); break;
                case "Weekly": next.setDate(next.getDate() + 7); break;
                case "Every2Weeks": next.setDate(next.getDate() + 14); break;
                case "Monthly": next.setMonth(next.getMonth() + 1); break;
                default: return next;
            }
        }
        return next;
    };

    const nextDate = getNextDate(data.startsAt, data.repetition);
    const dateFormatted = nextDate.toLocaleString('pl-PL', {
        day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', weekday:"long"
    });

    const wontBeAvaliable = nextDate < new Date();
    const avaiable = data.maxPeople > data.signedPeople || !data.maxPeople;

    async function OnButtonClick() {
        setIsLoading(true);
        const apiRes = await (signedUp ? afApiSignOut(data.id) : afApiSignUp(data.id));
        if (apiRes === true) {
            fUpdateData(prev => {
                const newState = [...prev];
                newState[index] = {
                    ...newState[index],
                    youSignedUp: !signedUp,
                    signedPeople: signedUp ? data.signedPeople - 1 : data.signedPeople + 1
                };
                return newState;
            });
        }
        else
            alert(apiRes.error);
        setIsLoading(false);
    }
    function RepetitionToPolish(value)
    {
        let out = "";
        switch (value)
        {
            case "None":
                out = "Tylko raz";
                break;
            case "Daily":
                out = "Codziennie";
                break;
            case "Weekly":
                out = "Co tydzień";
                break;
            case "Every2Weeks":
                out = "Co dwa tygodnie";
                break;
            case "Monthly":
                out = "Co miesiąc";
                break;
        }
        return out;
    }

    return (
        <Card className="mb-3 bg-dark text-white border-secondary shadow-sm m-1">
            <Card.Body>
                <div className="d-flex justify-content-between mb-2">
                    <h4 className="fw-bold mb-0">{data.name}</h4>
                </div>

                <p className="text-info small mb-2">Prowadzący: {data.instructor}</p>
                <p className="text-secondary small">{data.description}</p>

                <hr className="border-secondary" />

                <Row className="small mb-3">
                    <Col xs={7}>
                        <span className="text-secondary d-block text-uppercase" style={{fontSize: '10px'}}>Następne zajęcia:</span>
                        <span className="fw-bold">{dateFormatted} - {RepetitionToPolish(data.repetition)}</span>
                    </Col>
                    <Col xs={5} className="text-end">
                        <span className="text-secondary d-block text-uppercase" style={{fontSize: '10px'}}>Miejsca:</span>
                        <span className="fw-bold">{data.signedPeople} / {data.maxPeople || "∞"}</span>
                    </Col>
                </Row>

                <Button
                    variant={signedUp ? "outline-danger" : (avaiable && !wontBeAvaliable ? "primary" : "outline-warning")}
                    onClick={OnButtonClick}
                    disabled={isLoading || wontBeAvaliable || (!avaiable && !signedUp)}
                    className="w-100 fw-bold"
                >
                    {isLoading ?
                        "Przetwarzanie..." :
                        (
                            signedUp ?
                                "Wypisz się" :
                                (
                                    wontBeAvaliable ?
                                        "Te zajęcia już się odbyły" :
                                        (
                                            avaiable ?
                                                "Zapisz się" :
                                                "Brak wolnych miejsc"
                                        )
                                )
                        )
                    }
                </Button>
            </Card.Body>
        </Card>
    );
}


export default ClassItem;