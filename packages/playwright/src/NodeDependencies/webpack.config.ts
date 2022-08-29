
import path from "path";
import { Configuration } from "webpack";
import HtmlWebpackPlugin from "html-webpack-plugin";
import { ESBuildMinifyPlugin } from "esbuild-loader";

const enum EntryChunks {
    AxeCore = "axe-core",
    Report = "report"
}

const config: Configuration = {
    mode: "production",
    entry: {
        [EntryChunks.AxeCore]: path.join(__dirname, "./axe/index.axe-core.ts"),
        [EntryChunks.Report]: path.join(__dirname, "./report-src/index.tsx")
    },
    output: {
        path: path.join(__dirname, "dist"),
        filename: "index.[name].js"
    },
    resolve: {
        extensions: [".js", ".ts", ".tsx"]
    },
    performance: {
        hints: false
    },
    cache: {
        type: "filesystem"
    },
    optimization: {
        minimizer: [new ESBuildMinifyPlugin()]
    },
    module: {
        rules: [
            {
                test: /\.(ts|tsx)?$/,
                exclude: /node_modules/,
                use: "swc-loader",
            }
        ]
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: path.join(__dirname, "./report-src/index.html"),
            chunks: [EntryChunks.Report]
        })
    ]
}

export default config;