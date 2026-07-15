import { useState } from "react";
import type { ReactNode } from "react";
import { AppHeader } from "./AppHeader";
import { AppSidebar } from "./AppSidebar";

type DashboardLayoutProps = {
  children: ReactNode;
};

export function DashboardLayout({ children }: DashboardLayoutProps) {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  function handleOpenMenu() {
    setIsSidebarOpen(true);
  }

  function handleCloseMenu() {
    setIsSidebarOpen(false);
  }

  return (
    <div
      className="
        h-dvh
        w-full
        overflow-hidden
        bg-[#F5F8FC]

        lg:p-4
      "
    >
      <div
        className="
          mx-auto
          flex
          h-full
          w-full
          max-w-[1600px]
          overflow-hidden

          lg:gap-4
        "
      >
        <AppSidebar isOpen={isSidebarOpen} onClose={handleCloseMenu} />

        <div
          className="
            flex
            min-w-0
            flex-1
            flex-col
            overflow-hidden
            bg-[#F8FAFC]

            lg:rounded-[28px]
          "
        >
          <div
            className="
              shrink-0

              lg:px-8
            "
          >
            <AppHeader onOpenMenu={handleOpenMenu} />
          </div>

          <main
            className="
              flex-1
              overflow-y-auto
              px-4
              pb-6

              sm:px-6

              lg:px-8
              lg:pb-8
            "
          >
            {children}
          </main>
        </div>
      </div>
    </div>
  );
}
