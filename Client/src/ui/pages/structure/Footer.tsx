export default function Footer() {

    function getFooter() {
        return (
            <div className="my-dock rounded-xl bg-base-300 shadow-md flex flex-col md:flex-row items-center justify-center gap-4 p-4">
                <span>Jerne IF - Ingemanns alle 193 - 6700 Esbjerg - Tlf. 28442923</span>
                <a href="https://klubmodul.dk/default.aspx">
                    <img
                        src="https://jerneif.dk/cms/Clubjerneif/images/kmw.png"
                        alt="klubmodul"
                        className="h-10"
                    />
                </a>
            </div>
        );
    }

    return (
        <div className="flex justify-center shadow-md mt-10">
            <div className="w-full max-w-5xl bg-base-300 rounded-xl text-center">
                {getFooter()}
            </div>
        </div>
    );
}
