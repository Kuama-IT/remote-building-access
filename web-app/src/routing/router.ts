import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

const DashboardPage = () => import("../dashboard/dashboard.component.vue");
const AuthenticatePage = () =>
  import("../auth/pages/authenticate.component.vue");

const routes: Readonly<RouteRecordRaw[]> = [
  // TODO improve typings
  { path: "/authenticate", component: AuthenticatePage, name: "Authenticate" },
  { path: "/dashboard", component: DashboardPage, name: "Dashboard" },
];

export const appRouter = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to) {
    if (to.hash) {
      return {
        el: to.hash,
        behavior: "smooth",
      };
    }
    // always scroll to top
    return { top: 0 };
  },
});
