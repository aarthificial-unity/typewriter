module.exports = {
  tagFormat: "v${version}",
  branches: ["main"],
  plugins: [
    [
      "@semantic-release/commit-analyzer",
      {
        preset: "angular",
      },
    ],
    "@semantic-release/release-notes-generator",
    [
      "@semantic-release/changelog",
      {
        preset: "angular",
      },
    ],
    [
      "@semantic-release/npm",
      {
        npmPublish: false,
      },
    ],
    [
      "@semantic-release/git",
      {
        assets: ["package.json", "CHANGELOG.md"],
        message: "ci: release ${nextRelease.version} [skip ci]",
      },
    ],
    [
      "@semantic-release/github",
      {
        failComment: false,
      },
    ],
  ],
};
