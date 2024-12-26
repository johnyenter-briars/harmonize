from pathlib import Path

from PIL import Image


def crop_to_album_size(input_path: Path, output_path: Path, size=(500, 500)):
    # img = Image.open(input_path)

    # img_resized = img.resize(size, Image.Resampling.LANCZOS)

    # img_resized.save(output_path, 'JPEG')
    with Image.open(input_path) as img:
        width, height = img.size

        side_length = min(width, height)

        left = (width - side_length) // 2
        top = (height - side_length) // 2
        right = left + side_length
        bottom = top + side_length

        cropped_img = img.crop((left, top, right, bottom))

        cropped_img.save(output_path)


def convert_webp_to_jpeg(input_path: Path, output_path: Path):
    img = Image.open(input_path)

    img.convert('RGB').save(output_path, 'JPEG')
