
import { Label, Link, Separator, Stack, styled, Text, useTheme } from "@fluentui/react";
import React from "react";
import { RuleList } from "../rule-list";
import { defaultReportPageStyles } from "./report-page.styles";
import { getClassNames, IReportPageProps } from "./report-page.types";

const ReportPageBase = (props: IReportPageProps): JSX.Element => {
    const theme = useTheme();
    const classNames = getClassNames(props.styles, { theme });
    const dateTime = new Date(props.axeResults.timestamp);

    return (
        <>
            <Text as={"h1"} className={classNames.header}>Axe Report Page</Text>
            <Separator />
            <table>
                <tbody>
                    <tr>
                        <td>
                            <Label>Page Url:</Label>
                        </td>
                        <td>
                            <Link href={props.axeResults.url}>{props.axeResults.url}</Link>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <Label>Timestamp:</Label>
                        </td>
                        <td>
                            <Text>{dateTime.toLocaleDateString()} {dateTime.toLocaleTimeString()}</Text>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <Label>Violations</Label>
                        </td>
                        <td>
                            <Link href={props.axeResults.violations.length > 0 ? "#violations" : undefined}>{props.axeResults.violations.length}</Link>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <Label>Passes</Label>
                        </td>
                        <td>
                            <Link href={props.axeResults.passes.length > 0 ? "#passes" : undefined}>{props.axeResults.passes.length}</Link>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <Label>Incomplete</Label>
                        </td>
                        <td>
                            <Link href={props.axeResults.incomplete.length > 0 ? "#incomplete" : undefined}>{props.axeResults.incomplete.length}</Link>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <Label>Inapplicable</Label>
                        </td>
                        <td>
                            <Link href={props.axeResults.inapplicable.length > 0 ? "#inapplicable" : undefined}>{props.axeResults.inapplicable.length}</Link>
                        </td>
                    </tr>
                </tbody>
            </table>
            <Stack>
                <RuleList header={"Violations"} rules={props.axeResults.violations} />
                <RuleList header={"Passes"} rules={props.axeResults.passes} />
                <RuleList header={"Incomplete"} rules={props.axeResults.incomplete } />
                <RuleList header={"Inapplicable"} rules={props.axeResults.inapplicable} />
            </Stack>
        </>)
}

export const ReportPage = styled(ReportPageBase, defaultReportPageStyles);