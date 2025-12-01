import {Outlet} from "react-router-dom";
import Header from "./Header.tsx";
import Footer from "./Footer.tsx";
import Dock from "./Dock.tsx";

export default function Layout() {
    return (
        <div className="app-layout flex flex-col min-h-screen ">

            <div className="app-header">
                <Header />
                <div className="bg-base-200 shadow-md">
                    <div className="container mx-auto px-4 py-3">
                        <Dock />
                    </div>
                </div>
            </div>
            <div className="app-content flex-1">
               <Outlet />
            </div>
            <div className="app-footer"><Footer /></div>
        </div>
    )
}