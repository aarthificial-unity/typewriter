# Quickstart

> [!WARNING]
>
> The documentation is far from being complete and will be worked on in the
> following weeks. If you have any questions or need help, please reach out on
> our [Discord server].

## Installation

After [getting access to our GitHub organization][instructions], you will be
able to install Typewriter through Unity's Package Manager.

1. Click the "+" button in the top left corner and select "Add package from git
   URL...":

   ![The "add" dropdown in Unity's Package Manager](/images/package-manager-add.png)

2. Paste the following URL into the input field:

   ```text
   https://github.com/aarthificial-gamedev/typewriter.git
   ```

   ![The git URL input in Unity's Package Manager](/images/package-manager-path.png)

If you encounter any problems please refer to the [Unity's installation
guide][installation] or reach out on our [Discord server].

## Getting started

The package comes with a Visual Novel sample that shows how to implement a
simple text-based dialogue system with multiple choices. You can import it from
the Package Manager:

![The git URL input in Unity's Package Manager](/images/package-manager-samples.png)

Open the `SampleScene` located in the
`Assets/Samples/Typewriter/<version>/VisualNovel/Scenes` directory and press
play. You'll be presented with multiple buttons that demonstrate different types
of conversations:

- **Linear** showcases a simple, linear dialogue.
- **Choices** presents the player with a choice.
- **Contextual** changes the dialogue based on the context.

You can open the Typewriter Editor using `Tools > Typewriter` and explore the
entries that make up the dialogue. For clarity, each conversation has been
placed in its own table.

[Discord server]: https://www.patreon.com/posts/patron-only-53003221
[installation]: https://docs.unity3d.com/Manual/upm-ui-giturl.html
[instructions]: https://www.patreon.com/aarthificial
