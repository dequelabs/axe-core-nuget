import React from "react";
import { render, screen } from "@testing-library/react";
import { RuleResult } from "../rule-result";

describe("<RuleResult />", () => {

    it("Should render rule information as expected", () => {
        const description = "Ensures the contrast between foreground and background colors meets WCAG 2 AA contrast ratio thresholds";
        const id = "color-contrast";
        const help = "Ensures the contrast between foreground and background colors meets WCAG 2 AA contrast ratio thresholds";
        const helpUrl = "https://dequeuniversity.com/rules/axe/4.4/color-contrast?application=axeAPI";
        const tags = ["cat.color", "wcag143"];

        render(<RuleResult index={0} result={{
            description,
            id,
            help,
            helpUrl,
            tags,
            nodes: []
        }}/>)

        const expectedTitle = id;

        expect(screen.getByText(expectedTitle)).toBeInTheDocument();
        expect(screen.getByText(description)).toBeInTheDocument();
        expect(screen.getByText(helpUrl)).toBeInTheDocument();
    })
})