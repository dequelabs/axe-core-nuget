import { Label, Link, Stack, styled, Text, useTheme } from "@fluentui/react";
import React from "react";
import { DefaultRuleResultStyles } from "./rule-result.styles";
import { IRuleResultProps, getClassNames } from "./rule-result.types";

const RuleResultBase = (props: IRuleResultProps): JSX.Element => {
    const theme = useTheme();
    const classNames = getClassNames(props.styles, { theme });
    
    return(
    <>
        <Text as={"h3"} className={classNames.ruleName}>{props.result.id}</Text>
        <Text>{props.result.description}</Text>
        <Stack horizontal verticalAlign="center">
            <Label>Tags:</Label>
            {props.result.tags.map(v => <Text key={v}>{v}</Text>)}
        </Stack>
        <Stack horizontal verticalAlign="center">
            <Label>Help:</Label>
            <Link href={props.result.helpUrl}>{props.result.helpUrl}</Link>
        </Stack>
        {
            props.result.impact && (<Stack horizontal verticalAlign="center">
            <Label>Impact:</Label>
            <Text>{props.result.impact}</Text>
        </Stack>)
        }   
    </>)
}

export const RuleResult = styled(RuleResultBase, DefaultRuleResultStyles);