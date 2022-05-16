
import { Label, Stack, styled, Text, useTheme } from "@fluentui/react";
import React from "react";
import { RuleList } from "../rule-list";
import { defaultReportPageStyles } from "./report-page.styles";
import { getClassNames, IReportPageProps } from "./report-page.types";

const ReportPageBase = (props: IReportPageProps): JSX.Element => {
    const theme = useTheme();
    const classNames = getClassNames(props.styles, { theme });

    return (
        <>
            <Text as={"h1"} className={classNames.header}>Axe Report Page</Text>
            <Stack>
                <Stack horizontal verticalAlign="center" tokens={{ childrenGap: 7 }}>
                    <Label>Page Url: </Label>
                    <Text>{props.axeResults.url}</Text>
                </Stack>
                <Stack horizontal verticalAlign="center" tokens={{ childrenGap: 7 }}>
                    <Label>Timestamp: </Label>
                    <Text>{props.axeResults.timestamp}</Text>
                </Stack>
                <RuleList header={"Violations"} rules={props.axeResults.violations} />
                <RuleList header={"Passes"} rules={props.axeResults.passes} />
                <RuleList header={"Incomplete"} rules={props.axeResults.incomplete} />
                <RuleList header={"Inapplicable"} rules={props.axeResults.inapplicable} />
            </Stack>
        </>)
}

export const ReportPage = styled(ReportPageBase, defaultReportPageStyles);