import sys
import jwt
import time

appId = sys.argv[1]
appKey = sys.argv[2]
appTokenSeconds = sys.argv[3]
appTokenEncryption = sys.argv[4]

timeEpochSeconds = int(time.time())
appTokenPayload = {
  'iss': appId,
  'iat': timeEpochSeconds,
  'exp': timeEpochSeconds + appTokenSeconds
}

appToken = jwt.encode(appTokenPayload, appKey, algorithm=appTokenEncryption)
print(appToken.decode())
