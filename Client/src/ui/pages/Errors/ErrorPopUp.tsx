import { errorAtom } from "@core/atoms/error";
import { useAtom } from "jotai";
import { use, useEffect } from "react";
import { set } from "zod";

export default function ErrorPopUp() {
    const [error, setError] = useAtom(errorAtom);

    useEffect(() => {
        if (error) {
            // Show pop-up logic here
            alert(`Error: ${error}`);
            setError(null);
            // setTimeout(() => {
            // }, 3000);
        }
    }, [error]);
    return (
        <div>
            {/* Error Pop Up Implementation */}
        </div>
    );
}