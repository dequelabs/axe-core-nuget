
import React from "react";
import ReactDom from "react-dom";
import { ReportPage } from "./components";
import { AxeResults } from "./models";

declare global {
    interface Window { AxeResults?: AxeResults; }
}

const reactDivId = "root";
const axeResults = window.AxeResults;

ReactDom.render(axeResults !== undefined && axeResults !== null
    ? <ReportPage axeResults={axeResults}/>
    : <a>Error getting results.</a>, document.getElementById(reactDivId));
