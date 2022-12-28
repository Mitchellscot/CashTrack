module.exports = {
	env: {
		browser: true,
		es2021: true,
	},
	extends: ['xo', 'prettier'],
	overrides: [
		{
			extends: ['xo-typescript', 'prettier/prettier'],
			files: ['*.ts', '*.tsx'],
		},
	],
	ignorePatterns: ['**/*.js'],
	parserOptions: {
		ecmaVersion: 'latest',
		sourceType: 'module',
		project: './tsconfig.json',
		tsconfigRootDir: __dirname,
	},
	plugins: ['prettier'],
	rules: {
		'@typescript-eslint/prefer-for-of': 'off',
		'object-curly-spacing': 'off',
		'@typescript-eslint/object-curly-spacing': 'off'
	},
};
