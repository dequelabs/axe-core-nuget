import { FontWeights } from "@fluentui/react";
import { IRuleListStyleProps, IRuleListStyles } from "./rule-list.types";

const DefaultRuleListStyles = (props: IRuleListStyleProps): IRuleListStyles => ({
    ruleListHeader: {
        ...props.theme.fonts.xxLarge,
        fontWeight: FontWeights.semibold
    },
})

export { DefaultRuleListStyles }