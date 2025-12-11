import {Outlet, useLocation} from "react-router-dom";
import {useEffect, useRef} from "react";
import Header from "./Header.tsx";
import Footer from "./Footer.tsx";
import Dock from "./Dock.tsx";

export default function Layout() {
    const stickyDockRef = useRef<HTMLDivElement>(null);
    const location = useLocation();

    // Scroll to top on route change
    useEffect(() => {
        window.scrollTo(0, 0);
    }, [location.pathname]);

    useEffect(() => {
        const handleScroll = () => {
            if (stickyDockRef.current) {
                if (window.scrollY > 100) {
                    stickyDockRef.current.classList.remove("shadow-md");
                    stickyDockRef.current.classList.add("shadow-amber-900");
                } else {
                    stickyDockRef.current.classList.remove("shadow-amber-900");
                    stickyDockRef.current.classList.add("shadow-md");
                }
            }
        };

        window.addEventListener("scroll", handleScroll);
        
        // Cleanup function to remove listener when component unmounts
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    return (
        <div className="app-layout flex flex-col min-h-screen">
            {/* Header */}
            <div className="app-header">
                <Header />
            </div>
            {/* Sticky Dock */}
            <div ref={stickyDockRef} className="sticky top-0 z-50 bg-base-200 shadow-md">
                <div className="container mx-auto px-4 py-3">
                    <Dock />
                </div>
            </div>
            <div className="app-content flex-1 py-17">
               <Outlet />
            </div>
            <div className="app-footer mt-8">
                <Footer />
            </div>
        </div>
    )
}