// src/components/Layout.jsx
import Footer from "./Footer";
import { useState } from "react";
import Header from "./Header";
import Sidebar from "./Sidebar";

export default function Layout({ children }) {

  return (
    <>
      <Header />
      <div id="layoutSidenav">
        <Sidebar />
        <div id="layoutSidenav_content">
          <main>
            {children}
          </main>
          <Footer />
        </div>
      </div>

    </>
  );
}
