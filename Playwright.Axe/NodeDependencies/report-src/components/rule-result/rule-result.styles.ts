import { FontWeights } from "@fluentui/react";
import { IRuleResultStyleProps, IRuleResultStyles } from "./rule-result.types";

const DefaultRuleResultStyles = (props: IRuleResultStyleProps): IRuleResultStyles => ({
    ruleName: {
        ...props.theme.fonts.large,
        fontWeight: FontWeights.semibold
    }
})

export { DefaultRuleResultStyles }