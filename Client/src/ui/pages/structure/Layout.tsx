import {Outlet} from "react-router-dom";
import Header from "./Header.tsx";
import Footer from "./Footer.tsx";

export default function Layout() {
    return (
        <div className="app-layout flex flex-col min-h-screen ">

            <div className="app-header">
                <Header />
            </div>
            <div className="app-content mb-20">
               <Outlet />
            </div>
            <div className="app-footer"><Footer /></div>
        </div>
    )
}