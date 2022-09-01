import { Counter } from "./components/Counter";
import { Events } from "./components/Event/Events";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: '/events',
    element: <Events />
  }
];

export default AppRoutes;
