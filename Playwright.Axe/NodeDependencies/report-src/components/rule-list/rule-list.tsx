
import React from "react";
import { getClassNames, IRuleListProps } from "./rule-list.types";
import { styled, Text, useTheme } from "@fluentui/react";
import { DefaultRuleListStyles } from "./rule-list.styles";
import { RuleResult } from "../rule-result";

const RuleListBase = (props: IRuleListProps): JSX.Element => {
    const theme = useTheme();
    const classNames = getClassNames(props.styles, { theme });
    
    return (
    <>
        <Text as={"h2"} className={classNames.ruleListHeader}>{props.header}</Text>
        {props.rules.map((v) => <RuleResult key={v.id} result={v} />)}
    </>)
}

export const RuleList = styled(RuleListBase, DefaultRuleListStyles);