# Byte City desktop playtest

Double-click **Play Byte City.command** on the Desktop to start the local server and game, then press **Play** and choose your robot name. In Unity, use **Byte City → Play locally**.

The original robot town, characters, music, and cosmetic art are retained. Players, visitors, and town NPCs now have subtle pixel contact shadows. Guest play skips the old employer registration. First play retains the intro, Sensei’s flag and inventory lesson, and the town tour. After completing the tour, later sessions enter town directly; Chat → Sensei lesson replays the tutorial. Guests start with 1,000 practice coins and six starter cards. Progress currently lasts for the session; the old account service is bypassed.

## Controls

- WASD / arrows: move. Hold movement for 0.75 seconds, or hold Shift, to sprint at 1.65× speed. The directional pad remains available on mobile.
- **E** or the contextual button: interact with the nearby character, door, or game.
- Conversations: **Space**, **Enter**, or click to reveal a line; press again to continue.
- Card browser: left/right arrows or the on-screen buttons; **Esc / Done** returns to Deckmaster.
- Pipes: click a pipe to rotate it, then **Test flow**. Follow the arrows from source to sink.
- Wires: drag each numbered wire to its matching socket.
- F1: town; F2: pipes; F3: wires; F4: laboratory (on some Macs hold Fn).
- **C / Chat**: open town chat and connection settings; Esc closes the panel.
- Your name appears above your robot. Foreground scenery reveals a pale local silhouette; nearby interactive objects pulse and glint.
- Shop purchases and achievements use nonblocking notifications that slide in and fade away.
- Talk to Deckmaster in the laboratory, then choose **Challenge Deckmaster**. The Card Jitsu interaction also opens his match.

## Multiplayer

Up to eight people can explore the same town and see one another's movements, outfits, and room chat. Puzzles and their rewards are individual. Deckmaster Card Jitsu is a solo match. Town rooms do not provide synchronized player-versus-player card matches yet.

Run `tools/start-multiplayer.command` on the hosting computer. This requires Node.js and installs the locked `ws` dependency on first run. The service listens on port 8090.

On the host, use `ws://127.0.0.1:8090/socket` under **Connection settings**. Other computers on the same network use `ws://HOST-LAN-IP:8090/socket`. Everyone on the same host automatically joins the shared TOWN01 room after choosing a name. No room creation or code entry is needed. Connection settings remain available in Chat.

A browser build, when present in `Builds/Web`, is served at `http://HOST-LAN-IP:8090/` and automatically uses that server. It is intended for desktop browsers. Internet play requires a reachable HTTPS/WSS deployment or tunnel; a room code alone does not make a local server reachable outside your network.

The room server handles membership and chat limits. Movement is client-reported; this is a friends playtest, with no shared economy or competitive anti-cheat. Rooms disappear after their last player leaves.

## Rebuilding and verification

Unity 2022.3.45f1: **Byte City → Build Mac playtest** or **Build browser playtest**. Build reports go under `Builds`.

- EditMode: `MechanicsRegressionTests` covers pipe connectivity, generated board solutions, and score precision.
- PlayMode: `PuzzleRoundRegressionTests` covers complete puzzle sessions and restarting.
- Server: `cd backend/multiplayer && npm test` exercises two-player membership, movement, chat, room isolation, and disconnect cleanup.

## Artwork edit

`Assets/Images/UI_Sprites/Main-Menu/background.png` was edited with the built-in imagegen tool to remove the two baked-in Siemens tank labels. Prompt: “Remove both SIEMENS wordmarks at the bases of the two green tanks, replacing those lettered strips with plain dark metal bands matching the surrounding tank bases. Preserve the rest of the image: composition, pixel grid and block sizes, dark blue gray and muted green palette, lighting, workbenches, hanging tools, tiled floor, tanks and pipes. No new text, no logos, no other changes. Keep the original landscape aspect ratio. Retain crisp pixel edges without smoothing or repainting other areas.”

## Moving to a new host

This playtest needs no college account, old API credentials, or database. Run the new Node service on a host that supports persistent WebSockets, and route HTTPS and WSS to the same port. Set `PORT` if the platform supplies a port, and `WEB_ROOT` if the browser files are stored elsewhere. `/health` is the health check.

After generating `Builds/Web`, a container can be built from the Unity project root with `docker build -f backend/multiplayer/Dockerfile -t byte-city .`, then run with `docker run --rm -p 8090:8090 byte-city`. The container recipe is provided for your eventual host; it has not been deployed.

Browser players automatically connect to the host serving the page. Native players can enter its `wss://YOUR-HOST/socket` address under Connection settings. Display names are guest names, not unique registered accounts. Permanent profiles, authenticated usernames, and durable saves are future backend work.

## Current local play links

- This Mac: http://127.0.0.1:8090/
- Other computers on the same network: http://10.0.0.173:8090/ (the Mac must stay awake; its network address can change).
- No public deployment has been made.

## Verified September 10, 2026

The universal Mac app and WebGL build both completed with zero errors. The 11 EditMode mechanics checks and four PlayMode puzzle checks passed. The Node room integration test passed. A two-client Chrome playtest passed guest name entry, room creation/join, movement synchronization, chat, and disconnect cleanup, with no requests to the college API and no JavaScript exceptions. Browser screenshots and the result record are under `Captures/`.

## Deckmaster rules

Deckmaster commits to a card before the player chooses and uses the same card pool as the player’s owned collection. Each round offers the entire owned collection, sorted by element and power, with page arrows for collections larger than seven cards; playing never removes ownership. Heat beats Pressure, Pressure beats Electrical, and Electrical beats Heat using the original 10× element advantage. Same-element cards compare power. First to three wins takes the match, with a nine-round cap to prevent endless ties.

A match win awards 80 coins and a 5% chance of exactly one unowned card. Rewards are granted only once per completed match. Losses and draws cost no coins; leaving early earns no reward. Rewards currently last for the guest session, like other game progress.

## September 10 follow-up checks

14 EditMode checks passed, including sprint timing and fair opponent selection. Two additional PlayMode journey checks passed: all three Sensei flags plus inventory completion, and a victory granting coins and one card exactly once. Two Node integration checks passed, including automatic shared-town entry, the eight-player limit, and cleanup. Screenshots of the revised HUD, shop toast, and Deckmaster match are under `Captures/experience-*.png`.

The follow-up browser integration passed automatic TOWN01 entry for two named players, the restored Sensei town tour, sprint movement synchronization, chat, and disconnect cleanup with no legacy API requests or JavaScript exceptions (`Captures/auto-town-result.json`).

The final visual pass adds short hover/press feedback, eased panel reveals, a left-aligned shop information layout, brighter cool ambient lighting, warmer street lamps and firelight, and restrained equipment-light animation across the town, tutorial, laboratory, and casino. Pixel art, original intro, character art, and music remain intact. Final visual previews: `Captures/polish-lighting-final.png` and `Captures/polish-shop.png`.

Final browser build after the lighting and interface pass completed with zero errors. The existing desktop launcher starts the same local host and opens the Mac app.

The final universal Mac build also completed with zero errors. A smoke check of the final browser build passed name entry, automatic town connection, and opening Chat with no JavaScript exceptions.

## Dialogue and card layout repairs

World conversations use the original white pixel dialogue frame and font on a separate, screen-aligned canvas, with compact portraits and an explicit Reveal / Continue button. Space, Enter, or click first finishes the line and then advances it. The original intro and Sensei objectives remain.

Starter rewards and card collections reuse the original purple split card-book frame, with contained artwork, grouped stats, separate navigation, and an empty-pack message. Closing it returns to a new Deckmaster action menu. The world HUD retains the original circular icons and coin font, with labels, a contextual E interaction button, and the original lavender button artwork. Notifications reuse the existing purple panel sprite. Modal screens block movement and sprinting until closed.

Five relevant PlayMode checks passed: the two existing tutorial/reward journey checks, reveal-versus-advance input, card navigation and empty-state handling, and the complete Deckmaster introduction → starter cards → choices → movement flow. Editor previews include `Captures/ui-original-dialogue-final.png`, `Captures/ui-original-cards-final.png`, and `Captures/ui-original-cards-wide.png`.

The final UI styling uses existing project sprites, including `textbox2.png`, `cardmenu_without.png`, `default_button.png`, `square.png`, and the original circular HUD icons. Resizable sprite borders preserve their corners. The PlayMode checks also verify that dialogue and card screens use those original textures and the original pixel font.

The restored-artwork WebGL and universal Mac builds both completed with zero errors. The final browser check passed guest boot, automatic town connection, card navigation, and card → Deckmaster → HUD transitions at 1280×720 and 1024×768 with no browser errors (`Captures/ui-original-result.json`).

The Desktop launcher starts the Mac executable directly and avoids opening another copy when the game is already running. This bypasses a local LaunchServices issue observed when opening the rebuilt app bundle. Direct native startup was verified.


## September 12 arcade presentation

Card Jitsu now uses the original `cards.png` table frame, `cardback.png`, owned card artwork, pixel font, and `CardBackgroundTrack.wav`. The surrounding room's looping music pauses for the match and resumes on leaving. Choose from the full collection; the card travels to the table, Deckmaster waits one second, then his card slides in and flips before the clash scores. Element-colored sparks, short synthesized arcade tones, a brief table shake, match-point messaging, and a rematch button provide feedback. Input locks throughout resolution; leaving during a reveal cancels the unscored round.

Peculiar Pipes explicitly uses the original dark `PipeBackground.png` at its original color. Testing flow fills a coolant gauge and moves a mint pulse between pipe cells, with ascending tones, failure feedback, first-try recognition, and board-clear sparks. Source and sink also light up. The three boards now describe bringing the workshop, coolant pumps, and laboratory online.

A bottom-left schematic minimap uses a mint directional player arrow and labeled destination dots: blue for the lab/pipes, gold for the casino/wires/exits, and pink for the shop/cards. Its quiet grid shows relative positions without rendering scenery. Nearby destinations are named in the footer. It reuses the original panel and pixel font, hides during modal screens, and sits above the touch pad on phones.

Validation: all 15 selected EditMode mechanics checks and five PlayMode checks passed in Unity 2022.3.45f1. These cover full-deck selection, independent opponent commitment, delayed scoring, duplicate clicks, music restoration, leaving during a reveal, minimap cleanup, the original pipe background, puzzle completion/restarts, and one-time match rewards. Visual checks passed for the duel and minimap at widescreen and 1024×768. The chat launcher now sits beside the minimap. Screenshots are under `Captures/arcade-*.png`.

The Unity license was restored and runtime verification completed on September 12. Updated builds are generated with `ByteCityBuild`.


## Multiplayer follow-up: schematic map, touch browsers, robot collision

Remote avatars now have kinematic colliders matching the local robot's collision shape. They collide only with the local player's solid colliders, and ignore world triggers, scenery, and other remote colliders. Disconnecting removes the blocker. Sorting groups keep each robot's outfit together and order whole robots by their feet; nameplates and local silhouettes retain independent overlay sorting.

All three `ArcadePresentationTests` passed after these changes, including movement blocked by a visitor, movement restored after disconnect, and explicit exclusion of door triggers (`Captures/arcade-runtime-tests.txt`). Final editor screenshots: `Captures/minimap-symbols-town.png` and `Captures/robot-collision-layering.png`.

WebGL detects coarse-pointer touch browsers explicitly and retains the original directional pad. The page requests landscape use on phones, prevents scrolling gestures on the game canvas, and limits mobile render density. A Chromium touch-browser smoke check passed touch detection, tap-based guest entry, automatic TOWN01 connection, touch movement synchronized to the server, and no JavaScript exceptions. Screenshot: `Captures/phone-browser-smoke.png`. The canvas and footer fit the phone viewport separately; the map and chat launcher sit clear of all four movement arrows. Physical-phone behavior still needs a device playtest.

Local server: `http://10.0.0.173:8090/` on this Wi-Fi. The ngrok agent was started for port 8090; its current public URL is available from `http://127.0.0.1:4040/api/tunnels`. Public HTTPS health and two-client WSS room entry passed. The Mac must remain awake with the server and tunnel running. Logs: `/tmp/bytecity-town-server.log` and `/tmp/bytecity-ngrok.log`.

Final September 12 builds: Mac succeeded with zero errors (24.2 seconds); WebGL succeeded with zero errors (1 minute 27.5 seconds). The live Wi-Fi and ngrok pages serve this build. Reload browser clients and restart an already-open Mac app to pick up the schematic map, touch support, and avatar collision/layering fixes.
