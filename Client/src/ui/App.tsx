import './index.css'
import Layout from "./pages/structure/Layout.tsx";
import {createBrowserRouter, RouterProvider} from "react-router-dom";
import Home from "./pages/Home.tsx";
import Login from "./pages/Login.tsx";

const router = createBrowserRouter([
    {
        path: "/",
        element: <Layout />,
        children: [
            { path: "/", element: <Home /> },
            { path: "/login", element: <Login />}
        ],
    },
]);

function App() {
    return <RouterProvider router={router}/>
}

export default App

