
import React from "react";
import { getClassNames, IRuleListProps } from "./rule-list.types";
import { Stack, styled, Text, useTheme } from "@fluentui/react";
import { DefaultRuleListStyles } from "./rule-list.styles";
import { RuleResult } from "../rule-result";

const RuleListBase = (props: IRuleListProps): JSX.Element => {
    const theme = useTheme();
    const classNames = getClassNames(props.styles, { theme });
    
    if(props.rules.length > 0)
    {
        return (
            <>
                <Text as={"h2"} className={classNames.ruleListHeader}>{props.header}</Text>
                <Stack tokens={{ childrenGap: 10 }}>
                    {props.rules.map((v) => <Stack.Item><RuleResult key={v.id} result={v} /></Stack.Item>)}
                </Stack>
            </>)
    }

    return <></>
}

export const RuleList = styled(RuleListBase, DefaultRuleListStyles);