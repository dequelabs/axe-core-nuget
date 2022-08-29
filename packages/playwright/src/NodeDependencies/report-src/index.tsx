
import { ThemeProvider } from "@fluentui/react";
import React from "react";
import ReactDom from "react-dom";
import { ReportPage } from "./components";
import { AxeResults } from "./models";
import { defaultTheme } from "./theme";

declare global {
    interface Window { AxeResults?: AxeResults; }
}

const reactDivId = "root";
const axeResults = window.AxeResults;

ReactDom.render(axeResults !== undefined && axeResults !== null
    ? (<ThemeProvider theme={defaultTheme}><ReportPage axeResults={axeResults}/></ThemeProvider>)
    : <a>Error getting results.</a>, document.getElementById(reactDivId));
