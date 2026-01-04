import asyncio
import logging
import sys

from harmonize.vpn.status import vpn_status

logger = logging.getLogger('harmonize')


async def vpn_shutdown_background_service():
    while True:
        try:
            (connected, _) = vpn_status()

            if not connected:
                logger.warning('VPN is not running. Exiting')
                sys.exit(0)

            await asyncio.sleep(5)
        except Exception as e:
            logger.exception('Error in background service: %s', str(e))
        finally:
            pass
