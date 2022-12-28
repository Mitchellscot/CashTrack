const path = require('path');
const {CleanWebpackPlugin} = require('clean-webpack-plugin');
const webpack = require('webpack');
const ESLintPlugin = require('eslint-webpack-plugin');

const config = {
	entry: {
		settings: './Scripts/Settings/settings.ts',
		login: './Scripts/Account/login.ts',
		site: './Scripts/Site/site.ts',
		home: './Scripts/Home/home.ts',
		annual: './Scripts/Home/annual.ts',
		summary: './Scripts/Home/summary.ts',
		expenses: './Scripts/Expenses/expenses.ts',
		split: './Scripts/Expenses/split.ts',
		importExpenses: './Scripts/Import/importExpenses.ts',
		importIncome: './Scripts/Import/importIncome.ts',
		rules: './Scripts/Import/rules.ts',
		income: './Scripts/Income/income.ts',
		incomeCategories: './Scripts/IncomeCategories/incomeCategories',
		refund: './Scripts/Income/refund.ts',
		merchantDetail: './Scripts/Merchants/merchantDetail.ts',
		merchants: './Scripts/Merchants/merchants.ts',
		sources: './Scripts/Sources/sources.ts',
		sourceDetail: './Scripts/Sources/sourceDetail.ts',
		subCategories: './Scripts/SubCategories/subCategories.ts',
		subCategoryDetail: './Scripts/SubCategories/subCategoryDetail.ts',
		incomeCategoryDetail: './Scripts/IncomeCategories/incomeCategoryDetail.ts',
		mainCategories: './Scripts/MainCategories/mainCategories.ts',
		budget: './Scripts/Budget/budget.ts',
	},
	output: {
		filename: '[name].js',
		path: path.resolve(__dirname, 'wwwroot/js'),
	},
	plugins: [
		new CleanWebpackPlugin(),
		new webpack.ProvidePlugin({
			$: 'jquery',
		}),
		new ESLintPlugin({
			extensions: ['.ts', '.js'],
			exclude: 'node_modules',
		}),
	],
	module: {
		rules: [
			{
				test: /\.(ts|tsx)$/i,
				loader: 'ts-loader',
				exclude: ['/node_modules/'],
			},
		],
	},
	resolve: {
		extensions: ['.tsx', '.ts', '.js'],
	},
	watchOptions: {
		ignored: ['**/node_modules/', '*.js']
	},
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
