import pygame
import sys

# Inizializzazione di pygame
pygame.init()

# Creazione della finestra che si adatta alle dimensioni dello schermo
screen_info = pygame.display.Info()
screen_width = screen_info.current_w
screen_height = screen_info.current_h
screen = pygame.display.set_mode((screen_width, screen_height), pygame.RESIZABLE)
pygame.display.set_caption("Corpo Umano a Cartone")

# Definizione dei colori
LIGHT_YELLOW = (255, 255, 204)  # Giallo chiaro
ORANGE = (255, 140, 0)  # Arancione
VIVID_BLUE = (0, 0, 255)  # Blu vivace
play_button_color = (144, 238, 144)  # Verde chiaro
exit_button_color = (255, 100, 100)  # Rosso chiaro
dark_green = (0, 100, 0)  # Verde scuro
dark_red = (139, 0, 0)  # Rosso scuro

# Font per il testo
font = pygame.font.Font(None, 36)
title_font = pygame.font.Font(None, 72)  # Aumentato la grandezza del titolo
# Testo delle istruzioni
instructions_text_lines = [
    "Welcome to 'Let's Explore the Human Body'!",
    "In this game, you will learn about the human body",
    "by exploring different organs and systems.",
    "",
    "Instructions:",
    "1. Click 'Play' to start the game.",
    "2. Use your mouse to interact with the game.",
    "3. Answer questions and complete mini-games to earn points.",
    "4. Have fun and learn something new!"
]

# Definizione della posizione dei bottoni
play_button_rect = pygame.Rect(screen_width - 220, screen_height - 60, 100, 50)  # Posizione e dimensioni del bottone "PLAY"
exit_button_rect = pygame.Rect(screen_width - 110, screen_height - 60, 100, 50)  # Posizione e dimensioni del bottone "EXIT"

# Testo dei bottoni
play_button_text = font.render("PLAY", True, dark_green)  # Scritta in verde scuro
exit_button_text = font.render("EXIT", True, dark_red)  # Scritta in rosso scuro

# Loop principale
running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.MOUSEBUTTONDOWN:
            if exit_button_rect.collidepoint(event.pos):
                running = False
            elif play_button_rect.collidepoint(event.pos):
                # Aggiungi qui il codice per la funzionalità di PLAY
                print("Play button clicked")

    # Riempimento dello sfondo con un colore giallo chiaro
    screen.fill(LIGHT_YELLOW)

    # Disegno del titolo "INSTRUCTIONS"
    title_text = title_font.render("INSTRUCTIONS", True, ORANGE)
    screen.blit(title_text, (50, 20))

    # Disegno del testo delle istruzioni
    y_offset = 120
    for line in instructions_text_lines:
        line_text = font.render(line, True, VIVID_BLUE)
        screen.blit(line_text, (50, y_offset))
        y_offset += 50

    # Disegno dei bottoni con bordi tondeggianti
    pygame.draw.rect(screen, play_button_color, play_button_rect, border_radius=10)
    screen.blit(play_button_text, (play_button_rect.x + (play_button_rect.width - play_button_text.get_width()) // 2, play_button_rect.y + (play_button_rect.height - play_button_text.get_height()) // 2))
    pygame.draw.rect(screen, exit_button_color, exit_button_rect, border_radius=10)
    screen.blit(exit_button_text, (exit_button_rect.x + (exit_button_rect.width - exit_button_text.get_width()) // 2, exit_button_rect.y + (exit_button_rect.height - exit_button_text.get_height()) // 2))

    # Aggiornamento dello schermo
    pygame.display.flip()

# Chiusura di pygame
pygame.quit()
sys.exit()
