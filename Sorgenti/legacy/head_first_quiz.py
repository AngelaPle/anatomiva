import pygame
import sys
import time

# Inizializzazione di pygame
pygame.init()

# Creazione della finestra che si adatta alle dimensioni dello schermo
screen_info = pygame.display.Info()
screen_width = screen_info.current_w
screen_height = screen_info.current_h
screen = pygame.display.set_mode((screen_width, screen_height), pygame.RESIZABLE)
pygame.display.set_caption("Human Body Quiz")

# Definizione dei colori
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
LIGHT_BLUE = (173, 216, 230)
DARK_BLUE = (0, 0, 139)
GREEN = (0, 255, 0)
RED = (255, 0, 0)
LIGHT_GREEN = (144, 238, 144)
DARK_GREEN = (0, 100, 0)
LIGHT_RED = (255, 100, 100)
DARK_RED = (139, 0, 0)
LIGHT_ORANGE = (255, 165, 0)
DARK_ORANGE = (255, 140, 0)
VERY_LIGHT_RED = (255, 200, 200)  # rosso molto chiaro
CLICK_BLUE = (100, 149, 237)  # Cornflower blue


# Font per il testo
font = pygame.font.Font(None, 36)
title_font = pygame.font.Font(None, 48)
level_font = pygame.font.Font(None, 120)  # Aumentato la grandezza del titolo


# Domande e risposte
questions = [
    {"question": "What do you use to see?", "options": ["Ears", "Nose", "Eyes", "Mouth"], "answer": "Eyes"},
    {"question": "What do you use to hear?", "options": ["Eyes", "Nose", "Ears", "Mouth"], "answer": "Ears"},
    {"question": "What do you use to smell?", "options": ["Eyes", "Nose", "Ears", "Mouth"], "answer": "Nose"},
    {"question": "What do you use to taste?", "options": ["Eyes", "Nose", "Ears", "Mouth"], "answer": "Mouth"},
    {"question": "What do you use to talk?", "options": ["Eyes", "Nose", "Ears", "Mouth"], "answer": "Mouth"}
]

# Funzione per disegnare il testo centrato
def draw_text_centered(screen, text, font, color, y):
    text_surface = font.render(text, True, color)
    text_rect = text_surface.get_rect(center=(screen_width // 2, y))
    screen.blit(text_surface, text_rect)

# Funzione per disegnare le opzioni di risposta
def draw_options(screen, options, selected_option, clicked_index=-1):
    y_offset = 300
    option_rects = []
    button_width = 300
    button_height = 60
    for i, option in enumerate(options):
        if clicked_index == i:
            text_color = WHITE
        elif selected_option == i:
            text_color = GREEN
        else:
            text_color = BLACK

        option_text = font.render(option, True, text_color)
        option_rect = pygame.Rect((screen_width - button_width) // 2, y_offset, button_width, button_height)

        # Cambia colore se cliccato
        if clicked_index == i:
            bg_color = CLICK_BLUE  # blu evidenziato
        else:
            bg_color = LIGHT_BLUE

        pygame.draw.rect(screen, bg_color, option_rect, border_radius=10)
        screen.blit(option_text, (
            option_rect.x + (option_rect.width - option_text.get_width()) // 2,
            option_rect.y + (option_rect.height - option_text.get_height()) // 2
        ))
        option_rects.append(option_rect)
        y_offset += button_height + 20
    return option_rects


# Funzione per disegnare i bottoni
def draw_button(screen, text, x, y, width, height, button_color, text_color):
    pygame.draw.rect(screen, button_color, (x, y, width, height), border_radius=10)
    button_text = font.render(text, True, text_color)
    text_rect = button_text.get_rect(center=(x + width / 2, y + height / 2))
    screen.blit(button_text, text_rect)

# Loop principale
running = True
current_question = 0
selected_option = -1
score = 0
option_rects = []
show_score = False
score_display_time = 5  # Tempo in secondi per visualizzare il punteggio finale
show_first_level = True
first_level_display_time = 3  # Tempo in secondi per visualizzare "FIRST LEVEL"
first_level_start_time = time.time()

while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.KEYDOWN:
            if event.key == pygame.K_ESCAPE:
                running = False
        elif event.type == pygame.MOUSEBUTTONDOWN and not show_first_level:
            for i, rect in enumerate(option_rects):
                if rect.collidepoint(event.pos):
                    selected_option = i

                    # Disegna con evidenziazione
                    screen.fill(WHITE)
                    draw_text_centered(screen, questions[current_question]["question"], title_font, DARK_BLUE, 200)
                    draw_options(screen, questions[current_question]["options"], selected_option, clicked_index=i)
                    pygame.display.flip()

                    # Pausa breve per mostrare il clic visivamente
                    time.sleep(0.2)

                    # Controllo risposta
                    if questions[current_question]["options"][selected_option] == questions[current_question][
                        "answer"]:
                        score += 1
                    current_question += 1
                    selected_option = -1
                    if current_question >= len(questions):
                        show_score = True
                        score_start_time = time.time()


    if show_first_level:
        screen.fill(LIGHT_GREEN)  # Sfondo rosso chiaro
        draw_text_centered(screen, "FIRST LEVEL", level_font, DARK_GREEN, screen_height // 2 - 100)  # Testo rosso scuro
        draw_text_centered(screen, "BODY PART: Head", title_font, DARK_GREEN, screen_height // 2 + 20)
        if time.time() - first_level_start_time > first_level_display_time:
            show_first_level = False
    else:
        screen.fill(WHITE)
        if current_question < len(questions):
            # Disegno della domanda
            draw_text_centered(screen, questions[current_question]["question"], title_font, DARK_BLUE, 200)
            # Disegno delle opzioni di risposta
            option_rects = draw_options(screen, questions[current_question]["options"], selected_option)
        elif show_score:
            # Disegno del punteggio finale
            draw_text_centered(screen, f"Your final score: {score}/{len(questions)}", title_font, DARK_BLUE, screen_height // 2)
            if time.time() - score_start_time > score_display_time:
                # Disegno dei bottoni centrati in basso
                button_y = screen_height // 2 + 100
                draw_button(screen, "Next Level", screen_width // 2 - 100, button_y, 200, 50, LIGHT_GREEN, DARK_GREEN)
                if score < len(questions):
                    draw_button(screen, "Retry", screen_width // 2 - 100, button_y + 60, 200, 50, LIGHT_BLUE, DARK_BLUE)
                draw_button(screen, "Exit", screen_width // 2 - 100, button_y + 120, 200, 50, LIGHT_RED, DARK_RED)

                for event in pygame.event.get():
                    if event.type == pygame.MOUSEBUTTONDOWN:
                        mouse_x, mouse_y = event.pos
                        if screen_width // 2 - 100 <= mouse_x <= screen_width // 2 + 100:
                            if button_y <= mouse_y <= button_y + 50:
                                print("Next Level button clicked")
                                running = False  # Aggiungi qui il codice per il livello successivo
                            elif button_y + 60 <= mouse_y <= button_y + 110 and score < len(questions):
                                print("Retry button clicked")
                                current_question = 0
                                score = 0
                                show_score = False
                                show_first_level = True
                                first_level_start_time = time.time()
                            elif button_y + 120 <= mouse_y <= button_y + 170:
                                # Questo chiude la finestra quando premi Exit
                                running = False

    # Aggiornamento dello schermo
    pygame.display.flip()

    # Pausa tra le domande
    if current_question < len(questions) and selected_option != -1:
        time.sleep(2)  # Pausa di 2 secondi tra le domande

# Chiusura di pygame
pygame.quit()
sys.exit()
