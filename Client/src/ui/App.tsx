import './index.css';
import Layout from "./pages/structure/Layout.tsx";
import {createBrowserRouter, RouterProvider} from "react-router-dom";
import Home from "./pages/Home.tsx";
import Login from "./pages/Login.tsx";
import Register from "./pages/Register.tsx";
import Wallet from "./pages/Player/Wallet.tsx";
import MyBoards from "./pages/Player/Boards/MyBoards.tsx";
import CreateBoard from "./pages/Player/Boards/CreateBoard.tsx";
import RepeatingBoards from "./pages/Player/Boards/RepeatingBoards.tsx";
import GameHistory from "./pages/GameHistory.tsx";
import WinningBoards from "./pages/Player/History/WinningBoards.tsx";
import Profile from "./pages/Player/Profile.tsx";
import Dashboard from "./pages/Admin/Dashboard.tsx";
import AllPlayers from "./pages/Admin/Players/AllPlayers.tsx";
import WinningBoardsAdmin from "./pages/Admin/Games/WinningBoardsAdmin.tsx";
import Pending from "./pages/Admin/Transactions/Pending.tsx";
import AllTransactions from "./pages/Admin/Transactions/AllTransactions.tsx";
import AddPlayer from "./pages/Admin/Players/AddPlayer.tsx";
import Settings from "./pages/Admin/Settings.tsx";
import RequirePlayer from "./pages/structure/Auth/RequiredPlayer.tsx";
import RequireAdmin from "./pages/structure/Auth/RequiredAdmin.tsx";
import Forbidden403 from "./pages/Errors/Forbidden403.tsx";
import {CreateGameTemplate} from './pages/Admin/Games/CreateGameTemplate.tsx';
import {GamesOverview} from './pages/Admin/Games/GamesOverview.tsx';
import {StartGame} from './pages/Admin/Games/StartGame.tsx';
import ErrorPopUp from './pages/Errors/ErrorPopUp.tsx';
import Applications from "@ui/pages/Admin/Players/Applications.tsx";
import MyTickets from "@ui/pages/Player/MyTickets.tsx";
import Play from "@ui/pages/Games/Play.tsx";
import GamesDashboard from "@ui/pages/Games/GamesDashboard.tsx";

const router = createBrowserRouter([
    {
        path: "/",
        element: <Layout/>,
        children: [
            {path: "/", element: <Home/>},
            {path: "/login", element: <Login/>},
            {path: "/register", element: <Register/>},

            //Error page
            {path: "/403", element: <Forbidden403/>},

            // Player //
            {
                element: <RequirePlayer/>,
                children: [
                    {path: "/wallet", element: <Wallet/>},
                    {path: "/tickets", element: <MyTickets/>},

                    {path: "/boards", element: <MyBoards/>},
                    {path: "/boards/new", element: <CreateBoard/>},
                    {path: "/boards/repeating", element: <RepeatingBoards/>},

                    {path: "/games", element: <GamesDashboard/>},
                    {path: "/games/play/lotto/:gameId", element: <Play/>},

                    {path: "/history/games", element: <GameHistory/>},
                    {path: "/history/wins", element: <WinningBoards/>},


                    {path: "/profile", element: <Profile/>}
                ],
            },

            // Admin //
            {
                element: <RequireAdmin/>,
                children: [
                    {path: "/admin/dashboard", element: <Dashboard/>},

                    {path: "/admin/players", element: <AllPlayers/>},
                    {path: "/admin/players/register", element: <AddPlayer/>},
                    {path: "/admin/players/applications", element: <Applications/>},
                    {path: "/admin/games/overview", element: <GamesOverview/>},
                    {path: "/admin/games/start", element: <StartGame/>},
                    {path: "/admin/games/history", element: <GameHistory/>},
                    {path: "/admin/games/boards", element: <WinningBoardsAdmin/>},
                    // {path: "/admin/templates", element: <GameTemplatesList/>},
                    {path: "/admin/games/templates/create", element: <CreateGameTemplate/>},

                    {path: "/admin/transactions/pending", element: <Pending/>},
                    {path: "/admin/transactions/history", element: <AllTransactions/>},
                    // {path: "/admin/transactions/history", element: <AllTransactions/>},

                    {path: "/admin/settings", element: <Settings/>}
                ],
            },
        ],
    },
]);

function App() {
    return (
        <>
            <RouterProvider router={router}/>
            <ErrorPopUp/>
        </>
    )
}

export default App

