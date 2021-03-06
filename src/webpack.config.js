const path = require("path");
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const webpack = require('webpack');

const config = {
    entry: {
        settings: './Scripts/Settings/settings.ts',
        login: './Scripts/Account/login.ts',
        site: './Scripts/Site/site.ts',
        home: './Scripts/home/home.ts',
        expenses: './Scripts/Expenses/expenses.ts',
        split: './Scripts/Expenses/split.ts',
        importExpenses: './Scripts/Import/importExpenses.ts',
        importIncome: './Scripts/Import/importIncome.ts',
        income: './Scripts/Income/income.ts',
        refund: './Scripts/Income/refund.ts',
        merchantDetail: './Scripts/Merchants/merchantDetail.ts',
        merchants: './Scripts/Merchants/merchants.ts',
        sources: './Scripts/Sources/sources.ts',
        sourceDetail: './Scripts/Sources/sourceDetail.ts',
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, "wwwroot/js/"),
    },
    plugins: [
        new CleanWebpackPlugin(),
        new webpack.ProvidePlugin({
            $: 'jquery',
        }),
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

module.exports = (env, argv) => {
    if (argv.mode === 'development') {
        config.devtool = 'inline-source-map';
    }

    if (argv.mode === 'production') {
        //I don't know... uglify or something?
    }

    return config;
};
