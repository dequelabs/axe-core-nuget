import { classNamesFunction, IStyle, IStyleFunctionOrObject, ITheme } from "@fluentui/react";
import { AxeResults } from "../../models";

interface IReportPageProps {
    styles?: IStyleFunctionOrObject<IReportPageStyleProps, IReportPageStyles>;
    axeResults: AxeResults;
}

interface IReportPageStyleProps {
    theme: ITheme;
}

interface IReportPageStyles {
    header?: IStyle;
    subheader?: IStyle;
    ruleHeader?: IStyle;
}

const getClassNames = classNamesFunction<IReportPageStyleProps, IReportPageStyles>();

export { IReportPageProps, IReportPageStyleProps, IReportPageStyles, getClassNames }