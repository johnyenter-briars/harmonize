from pathlib import Path

from PIL import Image

from harmonize.const import AUDIO_IMAGE_COVER_ROOT, AUDIO_IMAGE_THUMBNAIL_ROOT


def crop_to_album_size(input_path: Path, output_path: Path):
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


def move_to_cover_folder(input_path: Path) -> Path:
    file_name = input_path.name

    full_path_to_image = AUDIO_IMAGE_COVER_ROOT / file_name

    with Image.open(input_path) as img:
        img.save(full_path_to_image)

    return full_path_to_image


def create_thumbnail(input_path: Path) -> Path:
    file_name = input_path.name

    full_path_to_image = AUDIO_IMAGE_THUMBNAIL_ROOT / file_name

    size = (150, 150)

    with Image.open(input_path) as img:
        img.thumbnail(size)

        img.save(full_path_to_image, format=img.format)
    return full_path_to_image
