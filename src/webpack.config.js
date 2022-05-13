const path = require("path");
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

const config = {
    entry: {
        login: './Scripts/Account/login.ts',
        site: './Scripts/Site/site.ts',
        home: './Scripts/home/home.ts',
        utils: './Scripts/Utility/utils.ts',
        expenses: './Scripts/Expenses/expenses.ts',
        split: './Scripts/Expenses/split.ts',
        importExpenses: './Scripts/Import/importExpenses.ts',
        importIncome: './Scripts/Import/importIncome.ts',
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
