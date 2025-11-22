export default function Footer() {

    function getFooter() {
        return (
            <div className="my-dock rounded-xl shadow-md bg-base-800 flex space-x-6 p-4">
                <span>Jerne IF - Ingemanns alle 193 - 6700 Esbjerg - Tlf. 28442923 </span>
                <a href="https://klubmodul.dk/default.aspx">
                    <img src="https://jerneif.dk/cms/Clubjerneif/images/kmw.png"
                         alt="klubmodul"/>
                </a>
            </div>
        );
    }

    return (
        <div
            className="flex flex-col md:flex-row items-center justify-between w-full max-w-5xl px-6 py-12 rounded-lg shadow-md">
            {getFooter()}
        </div>
    );
}