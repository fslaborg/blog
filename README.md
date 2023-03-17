# blog

The fslab blog

## Build the blog

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Any conda distribution (e.g. [anaconda](https://www.anaconda.com/) or [miniconda](https://docs.conda.io/en/latest/miniconda.html))

- `dotnet tool restore` to install the `fornax` tool that is used to build the website
- `conda create --name fslab-blog --file requirements.txt` to set up the conda environment

### develop locally in watch mode

- `conda activate fslab-blog` (this has to be done once for each development session)
- go into the `/src` folder: `cd src`
- `dotnet fornax watch` to start the site generator in watch mode
- go to `127.0.0.1:8080` to see the blog


## add content

### blog posts

posts are generated from the contents of folders in `/src/posts`.

To add a new post:
- add a folder with a **url-safe** name of your post to `/src/posts`
- create a `post_config.md` file. this file should *only* create metadata about your post, and must have this structure:
    ```
    ---
    title: <your post title>
    author: <your name>
    author_link: <a link>
    category: <post category>
    date: <YYYY-MM-DD>
    summary: <post summary>
    preview_image: images/<your_image_here.png>
    ---
    ```
    - `title` is the title of your post
    - `author` is the author of the post (most likely your name)
    - `author_link` is a link that will be associated with your name. You can for example link your github or twitter account here
    - `category` is one of `fsharp`, `datascience`, `advanced`
    - `date` is the date of submission in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)
    - `summary` is an optional short summary of the post. It is recommended to add for SEO.
    - `perview_image` is an optional image that will be shown on post previews. ideally a 2-by-1 or 3-by-1 image with a width of 1200px. It is recommended to add for SEO.

- create a `post.ipynb` file that contains your blogpost. This notebook will be parsed and rendered to a html site. **do not forget to save the notebook with cell output**, as the notebook will not be executed on site generation.


### graph gallery

To add a graph post:
- add a folder with a **url-safe** name of your post to `/src/graph-gallery`
- create a `graph_post_config.md` file. this file should *only* create metadata about your post, and must have this structure:
    ```
    ---
    title: <your post title>
    author: <your name>
    author_link: <a link>
    graph_category: <chart category>
    date: <YYYY-MM-DD>
    summary: <post summary>
    preview_image: images/<your_image_here.png>
    ---
    ```
    - `title` is the title of your post
    - `author` is the author of the post (most likely your name)
    - `author_link` is a link that will be associated with your name. You can for example link your github or twitter account here
    - `graph_category` is one of `basic`, `distribution`, `finance`, `3d`, `map`, `special` (corresponding to the categories of [plotly's docs](https://plotly.com/javascript/reference/))
    - `date` is the date of submission in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)
    - `summary` is an optional short summary of the post. It is recommended to add for SEO.
    - `perview_image` is an optional image that will be shown on post previews. ideally a 2-by-1 or 3-by-1 image with a width of 1200px. It is recommended to add for SEO.

- create a `<language>.ipynb` file that contains your graph post. This notebook will be parsed and rendered to a html site. Ideally, you provide both `fsharp.ipynb` and `csharp.ipynb`, but F#-only is okay as well. **do not forget to save the notebook with cell output**, as the notebook will not be executed on site generation.