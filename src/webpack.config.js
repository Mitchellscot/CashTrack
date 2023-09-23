const path = require('path');
const {CleanWebpackPlugin} = require('clean-webpack-plugin');
const webpack = require('webpack');
const ESLintPlugin = require('eslint-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");

const config = {
	entry: {
		settings: './Scripts/Settings/settings.ts',
		login: './Scripts/Account/login.ts',
		site: './Scripts/Site/site.ts',
		monthly: './Scripts/Home/monthly.ts',
		annual: './Scripts/Home/annual.ts',
		alltime: './Scripts/Home/alltime.ts',
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
		theme: './Scripts/Theme/theme.ts',
		addProfile: './Scripts/Settings/add-profile.tsx',
		charts: './node_modules/chart.js/dist/chart.umd.js',
		jQuery: './node_modules/jquery/dist/jquery.js',
		jqueryVal: './node_modules/jquery-validation/dist/jquery.validate.js',
		jqueryValUnob: './node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js',
		autocomplete: './node_modules/jquery-ui/ui/widgets/autocomplete.js',
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
			extensions: ['.ts', '.tsx'],
			exclude: 'node_modules',
			context: './Scripts',

		}),
	],
	module: {
		rules: [
			{
				test: /\.(ts|tsx)$/i,
				loader: 'ts-loader',
				exclude: ['/node_modules/']
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
		config.optimization = {
			minimize: true,
			minimizer: [(compiler) => {
				new TerserPlugin({
					parallel: true,
					terserOptions: {
						compress: true
					}
				}).apply(compiler)
			}]
		}
	}
	config.cache = argv.env.WEBPACK_WATCH ? {type: 'memory'} : {type: 'filesystem'};

	return config;
};
