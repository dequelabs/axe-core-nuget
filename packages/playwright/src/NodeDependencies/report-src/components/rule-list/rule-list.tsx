
import React, { useMemo } from "react";
import { getClassNames, IRuleListProps } from "./rule-list.types";
import { Link, Stack, styled, Text, useTheme } from "@fluentui/react";
import { DefaultRuleListStyles } from "./rule-list.styles";
import { RuleResult } from "../rule-result";
import { NumberSymbolIcon } from "@fluentui/react-icons-mdl2";

const RuleListBase = (props: IRuleListProps): JSX.Element => {
    const theme = useTheme();
    const classNames = getClassNames(props.styles, { theme });
    const id = useMemo(() => props.header.toLocaleLowerCase(), [props.header]);

    if(props.rules.length > 0)
    {
        return (
            <>
                <Stack horizontal verticalAlign="center">
                    <Text id={id} as={"h2"} className={classNames.ruleListHeader}>{props.header}</Text>
                    <Link href={`#${id}`} aria-labelledby={id}>
                        <NumberSymbolIcon />
                    </Link>
                </Stack>
                <Stack tokens={{ childrenGap: 50 }}>
                    {props.rules.map((v, index) => <Stack.Item><RuleResult index={index} key={v.id} result={v} /></Stack.Item>)}
                </Stack>
            </>)
    }

    return <></>
}

export const RuleList = styled(RuleListBase, DefaultRuleListStyles);