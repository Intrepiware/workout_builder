const path = require("path");

module.exports = {
  resolve: {
    extensions: [".js", ".ts", ".tsx", ".jsx"],
  },
  entry: {
    "home-index": "./src/home-index.tsx",
    "timing-index": "./src/timing-index.tsx",
    site: "./src/site.ts",
  },
  output: {
    path: path.resolve(__dirname, "../../wwwroot/js/pages"),
    filename: "[name].js",
  },
  module: {
    rules: [
      {
        use: {
          loader: "babel-loader",
        },
        test: /\.js$/,
        exclude: /node_modules/, //excludes node_modules folder from being transpiled by babel. We do this because it's a waste of resources to do so.
      },
      {
        use: {
          loader: "ts-loader",
        },
        test: /\.tsx?$/,
        exclude: /node_modules/,
      },
      {
        rules: [
          {
            test: /\.css$/i,
            use: ["style-loader", "css-loader"],
          },
        ],
      },
    ],
  },
};
