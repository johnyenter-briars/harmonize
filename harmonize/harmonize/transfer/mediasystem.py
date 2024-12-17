import os

import paramiko


def progress_callback(transferred, total):
    progress_percentage = (transferred / total) * 100
    print(f'Progress: {progress_percentage:.2f}% ({transferred}/{total} bytes)')


def transfer_file(ip: str, username: str, password: str, local_path: str, remote_path: str) -> None:
    try:
        ssh_client = paramiko.SSHClient()
        ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())

        ssh_client.connect(hostname=ip, username=username, password=password)

        sftp = ssh_client.open_sftp()

        file_size = os.path.getsize(local_path)
        transferred = 0

        sftp.put(local_path, remote_path, callback=progress_callback)

        print(f'File transferred successfully to {remote_path}')

        sftp.close()
        ssh_client.close()

    except Exception as e:
        print(f'An error occurred: {e}')
