import { Layout } from "@/components/layout";
import { Menu } from "@/routes/menu";
import { Outlet } from "react-router-dom"

export default function MainLayout() {
  return (
    <Layout>
      <Menu className="my-5 mx-5 flex gap-5" />
      <div>
        <Outlet />
      </div>
    </Layout>
  );
}