import { Layout } from "@/components/layout";
import { NavLink } from "react-router-dom";
import { Outlet } from "react-router-dom"

export default function MainLayout() {
  const settingStyles: ((props: { isActive: boolean; isPending: boolean; }) => React.CSSProperties) = (props) => (
    {
      color: props.isActive ? 'Highlight' : 'black'
    });

  return (
    <Layout>
      <div className="my-5 mx-5 flex gap-5">
        <NavLink to="/telemetry" style={settingStyles}>Телеметрия</NavLink>
        <NavLink to="/report" style={settingStyles}>Отчеты</NavLink>
      </div>
      <div>
        <Outlet />
      </div>
    </Layout>
  );
}