import { FontWeights } from "@fluentui/react";
import { IReportPageStyleProps, IReportPageStyles } from "./report-page.types";

const defaultReportPageStyles = (props: IReportPageStyleProps): IReportPageStyles => ({
    header: {
        ...props.theme.fonts.superLarge,
        fontWeight: FontWeights.bold
    },
    subheader: {
        ...props.theme.fonts.xxLarge,
        fontWeight: FontWeights.semibold
    },
    ruleHeader: {
        ...props.theme.fonts.large,
        fontWeight: FontWeights.semibold
    }
})

export { defaultReportPageStyles }