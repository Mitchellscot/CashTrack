const path = require("path");
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

const config = {
    entry: {
        home: './Scripts/home/home.ts',
        utils: './Scripts/Utility/utils.ts'
    },
    devtool: 'inline-source-map',
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, "wwwroot/js/"),
    },
    plugins: [
        new CleanWebpackPlugin()
    ],
    module: {
        rules: [
            {
                test: /\.(ts|tsx)$/i,
                loader: "ts-loader",
                exclude: ["/node_modules/"]
            }
        ],
    },
    resolve: {
        extensions: [".tsx", ".ts", ".js"],
    }
};

module.exports = config;
