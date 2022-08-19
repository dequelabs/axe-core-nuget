import { NumberSymbolIcon } from "@fluentui/react-icons-mdl2";
import { Label, Link, Stack, styled, Text, useTheme } from "@fluentui/react";
import React from "react";
import { DefaultRuleResultStyles } from "./rule-result.styles";
import { IRuleResultProps, getClassNames } from "./rule-result.types";

const RuleResultBase = (props: IRuleResultProps): JSX.Element => {
    const theme = useTheme();
    const classNames = getClassNames(props.styles, { theme, index: props.index });
    
    return(
    <div className={classNames.root}>
        <Stack horizontal verticalAlign="center">
            <Text id={props.result.id} as={"h3"} className={classNames.ruleName}>{props.result.id}</Text>
            <Link href={`#${props.result.id}`} aria-labelledby={props.result.id}>
                <NumberSymbolIcon />
            </Link>
        </Stack>
        <table>
            <tbody>
                <tr>
                    <td>
                        <Label>Description:</Label>
                    </td>
                    <td>
                        <Text>{props.result.description}</Text>
                    </td>
                </tr>
                <tr>
                    <td>
                        <Label>Help:</Label>
                    </td>
                    <td>
                        <Link href={props.result.helpUrl}>{props.result.helpUrl}</Link>
                    </td>
                </tr>
                <tr>
                    <td>
                        <Label>Tags:</Label>
                    </td>
                    <td>
                        {props.result.tags.map(v => <Text key={v}>{v}</Text>)}
                    </td>
                </tr>
                { props.result.impact && (<tr>
                    <td>
                        <Label>Impact:</Label>
                    </td>
                    <td>
                        <Text>{props.result.impact}</Text>
                    </td>
                </tr>)
                }
            </tbody>
        </table>
    </div>)
}

export const RuleResult = styled(RuleResultBase, DefaultRuleResultStyles);