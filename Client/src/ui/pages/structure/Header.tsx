import Dock from "./Dock.tsx";

export default function Header() {

    function getTitle() {
        return (
            <div className="flex items-center space-x-4 mb-6 md:mb-0">
                <img
                    src="https://jerneif.dk/cms/Clubjerneif/images/logo.png"
                    alt="Jerne IF"
                    className="w-16 h-16 object-cover"
                />
                <h1 className="text-4xl md:text-6xl font-bold text-default-400">
                    Lottery App
                </h1>
            </div>
        );
    }

    return (
        <div className="flex flex-col md:flex-col w-full max-w-5xl px-6 py-12 rounded-lg shadow-md space-y-6">
            {getTitle()}
            <Dock/>
        </div>
    );
}