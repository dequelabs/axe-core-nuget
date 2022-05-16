
import path from "path";
import { Configuration } from "webpack";
import HtmlWebpackPlugin from "html-webpack-plugin";

const config: Configuration = {
    mode: "production",
    entry: {
        path: path.join(__dirname, "./report-src/index.tsx")
    },
    output: {
        path: path.join(__dirname, "dist"),
        filename: "index.js"
    },
    resolve: {
        extensions: [".js", ".ts", ".tsx"]
    },
    module: {
        rules: [
            {
                test: /\.(ts|tsx)?$/,
                use: "ts-loader"
            }
        ]
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: path.join(__dirname, "./report-src/index.html")
        })
    ]
}

export default config;