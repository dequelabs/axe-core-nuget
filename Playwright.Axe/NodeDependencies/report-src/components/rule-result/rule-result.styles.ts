import { FontWeights } from "@fluentui/react";
import { IRuleResultStyleProps, IRuleResultStyles } from "./rule-result.types";

const DefaultRuleResultStyles = (props: IRuleResultStyleProps): IRuleResultStyles => ({
    ruleName: {
        ...props.theme.fonts.large,
        fontWeight: FontWeights.semibold
    },
    root: {
        backgroundColor: props.index % 2 === 0 ? "rgba(100, 100, 100, 0.1)" : "rgba(200, 200, 200, 0.1)"
    }
})

export { DefaultRuleResultStyles }