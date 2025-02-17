import subprocess


def vpn_status() -> tuple[bool, str]:
    output = subprocess.check_output(
        'nordvpn status | grep Status',
        shell=True,  # Let this run in the shell
        stderr=subprocess.STDOUT,
    )

    nord_status = str(output).split(': ')[1]

    country = ''
    connected = False
    if 'Connected' in nord_status:
        output = subprocess.check_output(
            'nordvpn status | grep Country',
            shell=True,  # Let this run in the shell
            stderr=subprocess.STDOUT,
        )

        country = str(output).split(': ')[1].replace('\\n', '').replace("'", '')
        connected = True

    return (connected, country)
